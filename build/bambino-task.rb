require "fileutils"

def bambino(*args, &block)
	body = lambda { |*args|
		task = Bambino.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end
    
class Bambino

    Phantom = "phantomjs.exe"
    Bambino = "bambino.coffee"

    attr_accessor :phantom_path, :bambino_path, :path, :create_runner, :runner_filename, :app_filter, 
                  :specs_path, :spec_filter, :require_path, :jasmine_path, :xml_output, :html_output, 
                  :teamcity_output, :output_path, :xml_output_path, :html_output_path, :output_filename, 
                  :xml_output_filename, :html_output_filename, :script_paths, :module_paths
    
    def initialize()
        @script_paths = []
        @module_paths = []
    end
    
    def add_script_paths(*paths)
        @script_paths.concat paths
    end
    
    def add_module_paths(*paths)
        @module_paths.concat paths
    end
    
    def run()
        command = []
        command << "\"#{File.expand_path(File.join(@phantom_path, Phantom))}\""
        command << "\"#{File.expand_path(File.join(@bambino_path, Bambino))}\""
        command << "\"#{@path}\""
        command << '--run'
        
        if @create_runner then command << '--create-runner' end
        if @runner_filename then command << '--runner-filename' << @runner_filename end
        if @app_filter then command << '--app-filter' << @app_filter end
        if @specs_path then command << '--specs-path' << "\"#{@specs_path}\"" end
        if @spec_filter then command << '--spec-filter' << @spec_filter end
        if @require_path then command << '--require-path' << "\"#{@require_path}\"" end
        if @jasmine_path then command << '--jasmine-path' << "\"#{@jasmine_path}\"" end
        if @xml_output then command << '--output' << 'xml' end
        if @html_output then command << '--output' << 'html' end
        if @teamcity_output then command << '--output' << 'teamcity' end
        if @output_path then command << '--output-path' << "\"#{@output_path}\"" end
        if @xml_output_path then command << '--xml-output-path' << "\"#{@xml_output_path}\"" end
        if @html_output_path then command << '--html-output-path' << "\"#{@html_output_path}\"" end
        if @output_filename then command << '--output-filename' << @output_filename end
        if @xml_output_filename then command << '--xml-output-filename' << @xml_output_filename end
        if @html_output_filename then command << '--html-output-filename' << @html_output_filename end
        if @script_paths.length > 0 then @script_paths.each{|script| command << "--script-path" << "\"#{script}\""} end
        if @module_paths.length > 0 then @module_paths.each{|mod| command << "--module-path" << mod} end
        
        command = command.join(" ")
        
        result = `#{command} 2>&1`
        puts result unless result.empty? 
        if $? != 0 then
            puts command
            error = "Jasmine tests failed: #{$?}."
            puts error
            fail error
        end
    end
end