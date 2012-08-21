require "fileutils"

def compile_coffeescript(*args, &block)
	body = lambda { |*args|
		task = CoffeeCompiler.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end
	
class CoffeeCompiler

    attr_accessor :path
    
    def run()
        errors = false
        
        Dir.glob(File.join(@path, '**/*.coffee')) do |path|
            puts "Compiling coffee script #{path}..."
            command = "coffee -b -c \"#{File.expand_path(path)}\""
            result = `#{command} 2>&1`
            puts result unless result.empty? 
            if $? != 0 then
                puts command
                puts "Coffeescript compiliation failed: #{$?}."
                errors = true
            end
        end
    
		fail "Coffeescript compiliation failed." unless !errors
    end
end