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
    attr_accessor :bottles_path, :package_type, :source_path, :output_path, :include_pdb, :overwrite, :project_file

    def run()
        command = []
        
        command << "\"#{@bottles_path || get_nuget_tool_path("Bottles", "BottleRunner.exe")}\""
        
        if @package_type == :zip then
            command << "create"
            command << "\"#{@source_path}\""
            command << "-o"
            command << "\"#{@output_path}\""
            if @include_pdb then command << "--pdb" end
            if @overwrite then command << "-f" end
        end
        
        if @package_type == :assembly then
            command << "assembly-pak"
            command << "\"#{@source_path}\""
            command << "-p"
            command << "\"#{@project_file}\""
        end
        
        command = command.join(' ')
        puts command
        fail "Fubu bottles create failed." unless system(command)
    end
    
    def get_nuget_tool_path(name, tool)
        path = Dir.glob("**/packages/#{name}**/tools/#{tool}")
        fail "#{name} nuget tool #{tool} could not be found under #{Dir.getwd}." unless !path.empty?
        return path.sort {|x,y| y <=> x } [0]
    end
end