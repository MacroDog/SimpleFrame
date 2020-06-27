function  hotfix(file)
    local old_file
    if package.loaded[file] then
        old_file = package.loaded[file]
    else
        print("null file"..file)
        return
    end 
    local err,ok= pcall(require, file)
    if not ok then 
        package.loaded[file]=old_file
        print(err)
        return
    end
    local new_file = package.loaded[file]
    update_table(new_file, old_file)
end

function update_table(new_table, old_table)
    assert("table"== type(new_table))
    assert("table"== type(old_table))
    for key, value in pairs(new_table) do
        local tp = type(value)
        if tp=="funtion" then
            update_func(value, old_table[key])
        elseif tp=="table" then
            update_table(value, old_table[key])
        end
    end
    local oldmateTabel = debug.getmetatable(old_table)
    local  newmateTable = debug.getmetatable(new_table)
    update_table(newmateTable, oldmateTabel)
end

function update_func(new_func, old_func)
    assert("function"==type(new_func))
    assert("function"==type(old_func))
    local oldupvalue = {}
    for index = 1, math.huge do
        local  name,value = debug.getupvalue(old_func, index)
        if not name then
            break
        else
            oldupvalue[name] = value
        end
    end

    for index = 1, math.huge do
        local name ,value = debug.getupvalue(new_func, index)
        if not name then
            break
        end
        local  oldvalue = oldupvalue[name]
        if  oldvalue then
            debug.setupvalue(new_func, index, oldvalue)
        end
    end
end