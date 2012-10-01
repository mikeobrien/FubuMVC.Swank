Swank = (function() {

    var swank = {};

    swank.ResourceTemplate = $('#swank-resource-template').html();

    swank.render = function(id) {
        $('.nav').find('li').removeClass('active');
        
        var content = $('#content');
        content.empty();
        
        if (!id) {
            content.html(swank.Specification.Comments);
            return;
        }
        
        var idParts = id.split(/^\/(.*?)(\/.*)$/, 3);
        var module = idParts[1];
        var resource = idParts[2];
        
        $('.nav').find("li[data-module='" + module + "']").addClass('active');
        
        content.html(Mustache.render(this.ResourceTemplate, { Name: resource }));
    };

    var getHash = function() {
        return window.location.hash.replace(/^#/, '');
    };

    $(window).bind('hashchange', function() {
        swank.render(getHash());
    });
    
    if (getHash()) $(window).trigger('hashchange');

    return swank;
})();