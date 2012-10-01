$(function () {
    
    Swank.ModuleTemplate = $('#swank-module-template').html();
    Swank.ResourceTemplate = $('#swank-resource-template').html();

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
            content.html(Mustache.render(this.ModuleTemplate, module));
        } else if (resource) {
            content.html(Mustache.render(this.ResourceTemplate, resource));
        } else {
            content.html('No module our resource specified.');
        }
    };

    var getHash = function() { return window.location.hash.replace(/^#/, ''); };

    $(window).bind('hashchange', function () { Swank.render(getHash()); });
    
    if (getHash()) $(window).trigger('hashchange');
    
});