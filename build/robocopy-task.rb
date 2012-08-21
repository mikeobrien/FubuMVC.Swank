require "fileutils"

def robocopy(*args, &block)
	body = lambda { |*args|
		task = Robocopy.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end
	
class Robocopy

    attr_accessor :source, :target, :exclude_dirs, :include_files, :log_path, :copy_mode, :retry_attempts, :retry_wait
    
	def initialize()
		@exclude_dirs = Array.new
		@include_files = Array.new
		@retry_attempts = 10
		@retry_wait = 30
	end
	
	def exclude_dirs(*dirs)
		@exclude_dirs.concat(dirs)
	end
	
	def include_files(*files)
		@include_files.concat(files)
	end
	
    def run()
        command = []
		
		command << "robocopy"
        command << "\"#{to_windows_path(@source)}\""
        command << "\"#{to_windows_path(@target)}\""
		
		case @copy_mode
		when :mirror
			command << "/MIR "
		when :updated
			command << "/M /S"
		end		

        if @exclude_dirs.length > 0 then 
			command << "/XD"
			command.concat(@exclude_dirs.map{|x| "\"#{to_windows_path(File.join(@source, x))}\""}) 
			command.concat(@exclude_dirs.map{|x| "\"#{to_windows_path(File.join(@target, x))}\""}) 
		end
		
        if @include_files.length > 0 then 
			command << "/IF "
			command.concat(@include_files.map{|x| "\"#{x}\""}) 
		end
		
        command << "/LOG+:\"#{to_windows_path(@log_path)}\""
        command << "/TEE"
		
		command << "/R:#{@retry_attempts}"
		command << "/W:#{@retry_wait}"
		
		log_directory = File.dirname(@log_path)
		if !Dir.exists?(log_directory) then 
			FileUtils.mkdir_p(log_directory)
		end
            
        error_handler = \
            lambda do |ok, res|
                       raise "Robocopy failed with exit " \
                             "code #{res.exitstatus}." \
                       if res.exitstatus > 8
                   end
		
		command = command.join(" ")
		puts command
        sh command, &error_handler 
    end
	
	private
	
	def to_windows_path(path)
		path.gsub("/", "\\")
	end
    
end
