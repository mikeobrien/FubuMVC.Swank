$(function () {

    Handlebars.registerHelper('methodColor', function (method) {
        switch (method.toLowerCase()) {
            case 'get': return 'blue';
            case 'post': return 'green';
            case 'put': return 'yellow';
            case 'update': return 'yellow';
            case 'delete': return 'red';
            default: return 'blue';
        }
    });

    Handlebars.registerHelper('formatUrl', function (url) {
        return url.replace(/(\{.*?\})/g, '<span class="highlight-text"><b>$1</b></span>');
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
        } else {
            content.html('No module our resource specified.');
        }
    };

    var getHash = function() { return window.location.hash.replace(/^#/, ''); };

    $(window).bind('hashchange', function () { Swank.render(getHash()); });
    
    if (getHash()) $(window).trigger('hashchange');
    
});