$(function () {
    
    var specification = Swank.Specification;
    var moduleTemplate = Handlebars.compile($('#swank-module-template').html());
    var resourceTemplate = Handlebars.compile($('#swank-resource-template').html());

    var render = function (id) {
            
        $('.nav').find('li').removeClass('active');
        
        var content = $('#content');
        content.empty();
        
        if (!id) {
            content.html(specification.Comments);
            return;
        }
        
        var idParts = id.split(/(.*?)(\/.*)/).filter(function (x) { return x; });
        var moduleName = idParts[0];
        var resourceName = idParts[1];

        var module = specification.Modules
                            .filter(function (x) { return x.Name === moduleName; })[0];
        var resource = (!module ? specification.Resources : module.Resources)
                            .filter(function (x) { return x.Name === resourceName; })[0];

        $('.nav').find("li[data-module='" + moduleName + "']").addClass('active');
        
        if (module && !resource) {
            content.html(moduleTemplate(module));
        } else if (resource) {
            content.html(resourceTemplate(resource));
            $('.endpoint-header').click(function () {
                $(this).next(".endpoint-body").slideToggle(500);
            });
            
            $('.show-json').click(function () {
                $('.show-json').addClass('active');
                $('.show-xml').removeClass('active');
                $(content).find(".json").show();
                $(content).find(".xml").hide();
                return false;
            });
            
            $('.show-xml').click(function () {
                $('.show-xml').addClass('active');
                $('.show-json').removeClass('active');
                $(content).find(".xml").show();
                $(content).find(".json").hide();
                return false;
            });

            $(window).resize(function () { $('.code-raw-container').remove(); });

            $('td.code').click(function () {
                var table = $(this).closest("table");
                var code = table.find('.code.json:visible, .code.xml:visible').map(function () { return $(this).text(); }).get().join("\r\n");
                var top = table.find('.code:visible:first');
                var bottom = table.find('.code:visible:last');
                var container = $('<div class="code-raw-container" style="' +
                    'top: ' + (top.offset().top + 1) + 'px;' +
                    'left: ' + (top.offset().left + 1) + 'px;' +
                    'width: ' + (top.outerWidth() - 2) + 'px;' +
                    'height: ' + ((bottom.offset().top - top.offset().top) + bottom.outerHeight() - 2) + 'px;' +
                    '"><textarea class="code-raw">' + code + '</textarea></div>').appendTo(content);
                $(container).find('textarea').select().blur(function (x) { $(x.target).parent().remove(); });
            });
        } else {
            content.html('No module our resource specified.');
        }
    };

    var getTypeDefinition = function (id, name, collection, depth, member, last, ancestors) {
        var definition = [];
        ancestors = ancestors || [];
        var isCyclic = ancestors.filter(function(x) { return x == id; }).length > 0;
        var type = specification.Types.filter(function (x) { return x.Id === id; })[0];
        last = typeof last != 'undefined' ? last : true;
        depth = depth || 0;

        if (collection && type && !isCyclic) {
            definition.push({ name: name, opening: true, collection: true, depth: depth, member: member, type: type });
            definition = definition.concat(getTypeDefinition(id, null, false, depth + 1, null, true, ancestors));
            definition.push({ name: name, closing: true, collection: true, depth: depth, last: last });
        } else if (collection) {
            definition.push({ name: name, itemName: type ? type.Name : id, opening: true, closing: true, collection: true,
                              depth: depth, last: last, member: member, type: type });
        } else if (type && !isCyclic) {
            definition.push({ name: name || type.Name, opening: true, depth: depth, member: member, type: type });
            for (var memberIndex in type.Members) {
                var lastMember = memberIndex == (type.Members.length - 1);
                var typeMember = type.Members[memberIndex];
                definition = definition.concat(getTypeDefinition(typeMember.Type, typeMember.Name, typeMember.Collection, 
                                               depth + 1, typeMember, lastMember, ancestors.concat(id)));
            }
            definition.push({ name: name || type.Name, closing: true, depth: depth, last: last });
        } else {
            definition.push({ name: name || id, depth: depth, member: member, type: type, last: last });
        }
        return definition;
    };

    var getFragmentDescription = function (description) {
        return !(!description.opening && description.closing) ? {
            type: (description.type ? description.type.Name :
                    (description.member ? description.member.Type : ''))
                  + (description.collection ? '[...]' : ''),
            defaultValue: description.member ? description.member.DefaultValue : null,
            collection: description.collection,
            required: description.member ? description.member.Required : null,
            optional: description.member ? !description.member.Required : null,
            comments: description.member ? description.member.Comments :
                        (description.type && !description.collection ? description.type.Comments : ''),
            options: description.member && description.member.Options &&
                description.member.Options.length > 0 ? description.member.Options : null
        } : null;
    };

    var getSampleValue = function (member) {
        if (member.Type == 'decimal' ||  member.Type == 'double' ||
            member.Type == 'float' || member.Type == 'unsignedByte' ||
            member.Type == 'byte' ||  member.Type == 'short' ||
            member.Type == 'unsignedShort' ||  member.Type == 'int' ||
            member.Type == 'unsignedInt' || member.Type == 'long' ||
            member.Type == 'unsignedLong') return member.DefaultValue || '0';
        else if (member.Type == 'boolean') return member.DefaultValue || 'false';
        else if (member.Type == 'guid') return member.DefaultValue || '\"00000000-0000-0000-0000-000000000000\"';
        else if (member.Type == 'dateTime') return member.DefaultValue || '\"10/26/1985 1:21:00 AM\"';
        else if (member.Type == 'string' ||
                 member.Type == 'char' ||
                 member.Type == 'base64Binary') return '\"' + (member.DefaultValue || '') + '\"';
        else return null;
    };

    var pad = function (x) { return new Array((x * 4) + 1).join(' '); };

    var getXmlFragment = function (description) {
        var sampleValue = description.member ? getSampleValue(description.member) : null;
        sampleValue = sampleValue ? sampleValue.replace(/\"/g, '') : '...';
        var element = '';
        if (!description.closing || (description.opening && description.closing)) {
            element += '<' + description.name + '>';
        }
        if (!description.opening && !description.closing) element += sampleValue;
        if (description.opening && description.closing) {
            element += '<' + description.itemName + '>' + sampleValue + '</' + description.itemName + '>';
        }
        if (!description.opening || (description.opening && description.closing)) {
            element += '</' + description.name + '>';
        }
        return pad(description.depth) + element;
    };

    var getJsonFragment = function (description) {
        var sampleValue = description.member ? getSampleValue(description.member) : null;
        var quote = function (x) { return "\"" + x + "\": "; };
        var element = '';
        if (description.opening) {
            if (description.depth > 0 && description.member) element += quote(description.name);
            element += description.collection ? '[' : '{';
        }
        if (description.opening && description.closing) {
            element += sampleValue ? sampleValue : '...';
        }
        if (description.closing) {
            element += description.collection ? ']' : '}';
            if (!description.last) element += ',';
        }
        if (!description.opening && !description.closing) {
            element += quote(description.name);
            element += sampleValue ? sampleValue : '{...}';
            if (!description.last) element += ',';
        }
        return pad(description.depth) + element;
    };

    var getTypeDescription = function (data) {
        return getTypeDefinition(data.Type, data.Name, data.Collection)
            .map(function (description) {
                return {
                    description: getFragmentDescription(description),
                    json: getJsonFragment(description),
                    xml: getXmlFragment(description)
                };
            });
    };
    
    Handlebars.registerHelper('methodColor', function (context) {
        switch (context.toLowerCase()) {
            case 'get': return 'blue';
            case 'post': return 'green';
            case 'put': return 'yellow';
            case 'update': return 'yellow';
            case 'delete': return 'red';
            default: return 'blue';
        }
    });
    
    Handlebars.registerHelper('formatUrl', function (context) {
        return context.replace(/(\{.*?\})/g, '<span class="highlight-text"><b>$1</b></span>');
    });

    Handlebars.registerHelper('when', function (predicate, options) {
        var declarations = '';
        for (var field in this) declarations += field + ' = this.' + field + ',';
        if (eval(declarations + predicate)) { return options.fn(this); }
    });
    
    Handlebars.registerHelper('colorizeJson', function (context) {
        return context.replace(/(\".*?\")/g, '<span style="color: #A31515">$1</span>');
    });

    Handlebars.registerHelper('colorizeXml', function (context) {
        return context.replace(/(\<)(.*?)(\>)/g, '<span style="color: #0000FF">$1</span><span style="color: #A31515">$2</span><span style="color: #0000FF">$3</span>');
    });

    Handlebars.registerHelper('typeDescription', function (context) {
        return getTypeDescription(this).map(function (x) { return context.fn(x); }).join('');
    });

    Handlebars.registerHelper('hasTypeDescription', function (context) {
        var data = this;
        return specification.Types.filter(function (x) { return x.Id === data.Type; }).length > 0 ? context.fn(data) : '';
    });

    var initialize = function() {
        var getHash = function() { return window.location.hash.replace(/^#/, ''); };

        $(window).bind('hashchange', function () { render(getHash()); });
    
        if (getHash()) $(window).trigger('hashchange');
    };

    initialize();
});