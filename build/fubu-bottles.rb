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
    attr_accessor :source_path, :output_path, :include_pdb, :overwrite

    def run()
		command = []
		
        command << "\"#{get_nuget_tool_path("Bottles", "BottleRunner.exe")}\""
        command << "create"
		command << "\"#{@source_path}\""
		command << "-o"
		command << "\"#{@output_path}\""
		if @include_pdb then command << "--pdb" end
		if @overwrite then command << "-f" end
		
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