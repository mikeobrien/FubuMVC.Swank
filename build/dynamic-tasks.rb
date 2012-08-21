def tasks(parent, children, &block)
	tasks = Hash[children.map{|x| "#{parent}_#{x.to_s}".gsub(" ", "_").to_sym}.zip(children)]
	task parent => tasks.keys
	tasks.each do |key, value|
		block.call key, value
	end
end

def multitasks(parent, children, &block)
	tasks = Hash[children.map{|x| "#{parent}_#{x.to_s}".gsub(" ", "_").to_sym}.zip(children)]
	multitask parent => tasks.keys
	tasks.each do |key, value|
		block.call key, value
	end
end