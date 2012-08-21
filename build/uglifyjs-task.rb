require "fileutils"

def uglifyjs(*args, &block)
	body = lambda { |*args|
		task = UglifyJs.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end
	
class UglifyJs

    attr_accessor :path
    
    def run()
        errors = false
        
        Dir.glob(File.join(@path, '**/*.js')) do |path|
            puts "Uglifying javascript #{path}..."
            command = "uglifyjs --overwrite \"#{File.expand_path(path)}\""
            result = `#{command} 2>&1`
            puts result unless result.empty? 
            if $? != 0 then
                puts command
                puts "Uglifying failed: #{$?}."
                errors = true
            end
        end
    
		fail "Uglifying javascript failed." unless !errors
    end
end