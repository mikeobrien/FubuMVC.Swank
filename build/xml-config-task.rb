require "yaml"
require "rexml/document"
include REXML

def xml_config(*args, &block)
	body = lambda { |*args|
		task = XmlConfig.new
		block.call(task)
		task.run
	}
	Rake::Task.define_task(*args, &body)
end

class XmlConfig

    attr_accessor :xml_file, :xml_root, :node_values, :yaml_files
    
    def initialize()
        @node_values = Array.new
		@yaml_files = Array.new
    end
    
	def set_node(xpath, value)
		@node_values << Node.new(xpath, value)
	end
	
	def from_yaml_file(path, *section)
		@yaml_files << YamlFile.new(path, *section)
	end
    
    def run()

		puts xml_file
		
		nodes = Array.new
		
		nodes.concat(@node_values)
		
		@yaml_files.each do |file|
			yaml = YAML::load_file(file.path)
			results = yaml
			file.section.each do |section|
				results = results[section]
			end
			nodes.concat(results.map{|x| Node.new(x.keys[0], x.values[0])})
		end
		
        document = Document.new File.new(@xml_file)
        document.context[:attribute_quote] = :quote
		
        nodes.each do |sourceNode| 
			xpath = "#{@xml_root}/#{sourceNode.xpath}"
			puts "#{xpath} = #{sourceNode.value}"
			targetNode = XPath.first(document, xpath)
            if (targetNode != nil && targetNode.class == REXML::Attribute)
                targetNode.element.attributes[targetNode.name] = sourceNode.value
            elsif (targetNode != nil && targetNode.class == REXML::Element)
                element = Document.new(sourceNode.value)
                element.context[:attribute_quote] = :quote
                targetNode.add_element(element)
            elsif (targetNode != nil)
                raise "Unsupported element type #{targetNode.class}"
            end
        end
		
        formatter = Formatters::Pretty.new
        File.open(@xml_file, 'w') do |result|
            formatter.write(document, result)
        end
    end
	
	class YamlFile
		attr_accessor :path, :section
		def initialize(path, *section)
			@path = path
			@section = section
		end
	end
    
    class Node
        attr_accessor :xpath, :value
        def initialize(xpath, value)
            @xpath = xpath
			@value = value
        end
    end
    
end
    
    