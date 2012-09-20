require "fileutils"

def create_fubu_bottle(*args, &block)
	body = lambda { |*args|
		task = CreateFubuBottle.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end
	
class CreateFubuBottle

    attr_accessor :project_path, :output_path

    def run()
		command = []
		
        command << "\"#{get_nuget_tool_path("Bottles", "BottleRunner.exe")}\""
        command << "assembly-pak"
		command << "\"#{@output_path}\""
		command << "-p"
		command << "\"#{@project_path}\""
		
		command = command.join(' ')
		puts command
		fail "Fubu bottles pack failed." unless system(command)
    end
    
    def get_nuget_tool_path(name, tool)
        path = Dir.glob("**/packages/#{name}**/tools/#{tool}")
        fail "#{name} nuget tool #{tool} could not be found under #{Dir.getwd}." unless !path.empty?
        return path[0]
    end
end