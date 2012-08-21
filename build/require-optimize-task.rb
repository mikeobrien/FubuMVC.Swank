require "fileutils"

def require_optimize(*args, &block)
	body = lambda { |*args|
		task = RequireOptimizer.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end
	
class RequireOptimizer

    attr_accessor :path, :main_file, :exclude_path_filter, :include_path_filter, :noop_plugins, :excluded_modules
    
    def noop_plugins(*plugins)
        @noop_plugins = plugins
    end
    
    def exclude_modules(*modules)
        @excluded_modules = modules
    end
    
    def run()
        errors = false
        
        Dir.glob(File.join(@path, "**/#{@main_file}")) do |path|
            errors = true unless optimize(path)
        end
    
		fail "Require.js optimizations failed." unless !errors
    end
    
    def optimize(path)
        main = read_file path
        
        return true unless contains_require main
        
        puts "Optimizing require.js module #{path}..."
        
        paths = get_paths(main).merge excluded_modules
        exclude_paths paths
        
        compiled_main = get_temp_file_path path, 'compiled'
        
        config = RequireBuildConfig.new
        config.name = File.basename(path, ".js")
        config.out = File.basename(compiled_main)
        config.base_url = File.expand_path(File.dirname(path))
        config.optimize = 'none'
        config.inline_text = 'true'
        config.paths = paths
        config.noop_plugins = @noop_plugins
        
        build_file_path = get_temp_file_path(path, 'build')
        config.write(build_file_path)

        command = "r.js.cmd -o \"#{File.expand_path(build_file_path)}\""
        result = `#{command} 2>&1`
        puts result unless result.empty? 
        if $? == 0 then
            File.delete path
            File.rename compiled_main, path
            File.delete build_file_path
            return true
        else
            puts command
            puts "Require.js optimizations failed: #{$?}."
            return false
        end
    end
    
    def read_file(path)
        data = nil
        File.open(path, "rb") do |file|
            data = file.read
        end    
        data
    end
    
    def contains_require(file)
        file.index(/require(.*)/)
    end
    
    def excluded_modules()
        Hash[@excluded_modules.map{|plugin| [plugin, 'empty:']}]
    end
    
    def exclude_paths(paths)
        paths.each{|name, path| paths[name] = 'empty:' unless module_excluded(name, path)}
    end
    
    def module_excluded(name, path)
        !path.index(@exclude_path_filter) or path.index(@include_path_filter)
    end
    
    def get_temp_file_path(path, name)
        File.join(File.dirname(path), get_temp_file(path, name))
    end
    
    def get_temp_file(path, name)
        File.basename(path, ".js") + ".#{name}.js"
    end

    def get_paths(file)
        paths_section = file.scan(/paths\s*:\s*\{\s*(.*?)\s*\}/m)
        if paths_section.length > 0 and paths_section[0].length > 0
            return Hash[paths_section[0][0].scan(/\s*[\"\']*(\w*)[\"\']*\s*:\s*[\"\'](.*?)[\"\']\s*,?/m)]
        end
        Hash.new
    end
end

class RequireBuildConfig

    attr_accessor :base_url, :name, :out, :paths, :optimize, :inline_text, :noop_plugins
    
    def initialize()
        @paths = Hash.new
        @excluded = Array.new
    end
    
    def write(path)
        File.open(path, 'w') do |config|  
            config.puts '({'
            config.puts "\tbaseUrl: '#{@base_url}',"
            config.puts "\tname: '#{@name}',"
            config.puts "\tout: '#{@out}',"
            config.puts "\tpaths: {"
            config.puts paths.map{|name, path| "\t\t#{name}: '#{path}'"}.join(",\r")
            config.puts "\t},"
            config.puts "\toptimize: '#{@optimize}',"
            config.puts "\tinlineText: #{@inline_text},"
            config.puts "\tonBuildWrite: #{noop(@noop_plugins)}"
            config.puts '})'
        end  
    end

    private
    
    def noop(plugins)
        "function (moduleName, path, contents) {
            if ([#{plugins.map{|plugin| "'#{plugin}'"}.join(', ')}].indexOf(moduleName) !== -1) {
                return \"define('\" + moduleName + \"', { load:function(){} });\";
            } else { return contents; }
        }"  
    end
end