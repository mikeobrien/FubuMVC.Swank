$(function () {

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

    Handlebars.registerHelper('yesNo', function (context) {
        return context ? 'Yes' : 'No';
    });

    Swank.ModuleTemplate = Handlebars.compile($('#swank-module-template').html());
    Swank.ResourceTemplate = Handlebars.compile($('#swank-resource-template').html());

    Swank.render = function (id) {
            
        $('.nav').find('li').removeClass('active');
        
        var content = $('#content');
        content.empty();
        
        if (!id) {
            content.html(Swank.Specification.Comments);
            return;
        }
        
        var idParts = id.split(/(.*?)(\/.*)/).filter(function (x) { return x; });
        var moduleName = idParts[0];
        var resourceName = idParts[1];

        var module = Swank.Specification.Modules
                            .filter(function (x) { return x.Name === moduleName; })[0];
        var resource = (!module ? Swank.Specification.Resources : module.Resources)
                            .filter(function (x) { return x.Name === resourceName; })[0];

        $('.nav').find("li[data-module='" + moduleName + "']").addClass('active');
        
        if (module && !resource) {
            content.html(this.ModuleTemplate(module));
        } else if (resource) {
            content.html(this.ResourceTemplate(resource));
            $('.endpoint-header').click(function () { $(this).next(".endpoint-body").slideToggle(500); });
            $('.show-json').click(function () { $(content).find(".json").show(); $(content).find(".xml").hide(); return false; });
            $('.show-xml').click(function () { $(content).find(".xml").show(); $(content).find(".json").hide(); return false; });
            
            //$('.copy-code').click(function () {
            //    console.log(window.clipboardData);
            //    if (window.clipboardData) {
            //        var code = $(this).closest("table").find('.json:visible, .xml:visible')
            //            .map(function (x) { return x.text(); }).join("\r\n");
            //        window.clipboardData.setData("Text", code);
            //    }
            //    return false;
            //});
        } else {
            content.html('No module our resource specified.');
        }
    };

    var getDefaultValue = function (member) {
        if (member.Type == 'decimal' ||
            member.Type == 'double' ||
            member.Type == 'float' ||
            member.Type == 'unsignedByte' ||
            member.Type == 'byte' ||
            member.Type == 'short' ||
            member.Type == 'unsignedShort' ||
            member.Type == 'int' ||
            member.Type == 'unsignedInt' ||
            member.Type == 'long' ||
            member.Type == 'unsignedLong') return member.DefaultValue || '0';
        else if (member.Type == 'boolean') return member.DefaultValue || 'false';
        else if (member.Type == 'guid') return member.DefaultValue || '\"00000000-0000-0000-0000-000000000000\"';
        else if (member.Type == 'dateTime') return member.DefaultValue || '\"10/26/1985 1:21:00 AM\"';
        else return '\"' + (member.DefaultValue || '') + '\"';
    };

    var getTypeDefinition = function (id, name, collection, depth, member, last) {
        var type = Swank.Specification.Types.filter(function (x) { return x.Id === id; })[0];
        last = typeof last != 'undefined' ? last : true;
        var definition = [];
        depth = depth || 0;

        if (collection && type) {
            definition.push({
                name: name,
                opening: true,
                collection: true,
                depth: depth,
                member: member
            });
            definition = definition.concat(getTypeDefinition(id, null, false, depth + 1));
            definition.push({
                name: name,
                closing: true,
                collection: true,
                depth: depth,
                last: last
            });
            return definition;
        } else if (collection) {
            definition.push({
                name: name,
                itemName: id,
                opening: true,
                closing: true,
                collection: true,
                depth: depth,
                member: member,
                defaultValue: getDefaultValue(member)
            });
            return definition;
        }

        if (type) {
            definition.push({
                name: name || type.Name,
                opening: true,
                depth: depth,
                member: member,
                type: type
            });
            for (memberIndex in type.Members) {
                var lastMember = memberIndex == (type.Members.length - 1);
                var typeMember = type.Members[memberIndex];
                definition = definition
                    .concat(getTypeDefinition(typeMember.Type, typeMember.Name,
                                typeMember.Collection, depth + 1, typeMember, lastMember));
            }
            definition.push({
                name: name || type.Name,
                closing: true,
                depth: depth,
                last: last
            });
        } else {
            definition.push({
                name: name || id,
                depth: depth,
                member: member,
                last: last,
                defaultValue: getDefaultValue(member)
            });
        }
        return definition;
    };

    var pad = function (x) { return new Array((x * 4) + 1).join(' '); };

    var getXmlFragment = function (description) {
        var defaultValue = description.defaultValue ? description.defaultValue.replace(/\"/g, '') : '';
        var element = '';
        if (!description.closing || (description.opening && description.closing)) {
            element += '<' + description.name + '>';
        }
        if (!description.opening && !description.closing) element += defaultValue;
        if (description.opening && description.closing) {
            element += '<' + description.itemName + '>' + defaultValue + '</' + description.itemName + '>';
        }
        if (!description.opening || (description.opening && description.closing)) {
            element += '</' + description.name + '>';
        }
        return pad(description.depth) + element;
    };

    var getJsonFragment = function (description) {
        var quote = function (x) { return "\"" + x + "\": "; };
        var element = '';
        if (description.opening) {
            if (description.depth > 0 && description.member) element += quote(description.name);
            element += description.collection ? '[' : '{';
        }
        if (description.opening && description.closing) {
            element += description.defaultValue;
        }
        if (description.closing) {
            element += description.collection ? ']' : '}';
            if (!description.last) element += ',';
        }
        if (!description.opening && !description.closing) {
            element += quote(description.name);
            element += description.defaultValue;
            if (!description.last) element += ',';
        }
        return pad(description.depth) + element;
    };

    var getTypeDescription = function (data) {
        return getTypeDefinition(data.Type, data.Name, data.Collection)
            .map(function (description) {
                return {
                    member: description.member,
                    type: description.type,
                    json: getJsonFragment(description),
                    xml: getXmlFragment(description)
                };
            });
    };

    Handlebars.registerHelper('typeDescription', function (context, options) {
        return getTypeDescription(this).map(function (x) { return context.fn(x); }).join('');
    });

    var getHash = function() { return window.location.hash.replace(/^#/, ''); };

    $(window).bind('hashchange', function () { Swank.render(getHash()); });
    
    if (getHash()) $(window).trigger('hashchange');
    
});