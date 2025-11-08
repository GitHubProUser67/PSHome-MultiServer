namespace WebAPIService.GameServices.PSHOME.OHS
{
    public static class LUA2XmlCode
    {
        public const string lua2xml = @"--[[--------------------------------]]--
--[[ XmlConvert.lua	(modified)  	]]--
--[[		                        ]]--
--[[ GitHubProUser67		        ]]--
--[[ 16/02/2025		                ]]--
--[[--------------------------------]]--

XmlConvert =
(
	function()
		-- Private Variables
		-- Private Functions
		local function omnitype( x )
		    local t = type(x)
		    return (t ~= 'userdata') and t or Type(x)
		end

		-- Courtesy of Mr. Slaney
		local function stringSplit( str, convertFunc )
			local convertFunc = convertFunc or function(matchStr) return matchStr end
		    local results = {}
		    local terminated = false
		    for match, delimiter in str:gmatch('([^,]*)([,]?)') do
		        -- If both are """" ignore it.
		        if match ~= delimiter then
		            results[#results+1] = convertFunc(match)
		            terminated = delimiter == ','
		        end
		    end
		    -- There was a trailing comma which implies a blank.
		    if terminated then
		        results[#results+1] = """"
		    end
		    return results
		end

		local function tableCount(tbl)
            local count = 0
            for _ in pairs(tbl) do
                count = count + 1
            end
            return count
        end

		-- Public Variables
		local self = {}

		-- Returns a valid XML string, at minimum open and close tags.
		-- ipairs maintain their order, pairs are sorted alphabetically in order to facilitate nicer file diffing.
		-- NOTE: Currently all ipairs entries are considered to be consecutive numbers even if they aren't. Order is maintained, but specific indexes are NOT.
		self.LuaToXml = function( tbl, tag, tab )
			-- TODO: Handle escape characters in strings: (<, >, "", &).
			-- TODO: Check ipairs keys are ints (math.floor(x) == x), not just numbers.
			-- TODO: Handle number-strings (e.g. ""1""), as this is a pairs key, NOT an ipairs key.

			local tbl = tbl or {}
			local _t 	= {}
			local tag = tag or 'lua'
			local tab = tab or 1

			local convert =
			{
				string = function( val )
					-- Workaround for lack of Vector4 in classic lua interpreters.
					local pattern = ""^([+-]?[0-9]*%.?[0-9]+),([+-]?[0-9]*%.?[0-9]+),([+-]?[0-9]*%.?[0-9]+),([+-]?[0-9]*%.?[0-9]+)$""
					local x, y, z, w = val:match(pattern)
					-- If we found the vector pattern, process it
					if x and y and z and w then
						local components = {x, y, z, w}
						return table.concat(components, ','), 'vec'
					else
					  return tostring(val)  -- Default string conversion
					end
				end,
				boolean	   = function( val ) return tostring(val), 'bool' end,
				number 	   = function( val ) return math.floor(val) == val and string.format('%d', val) or tostring(val), 'num' end,
				vector4	   = function( val ) return table.concat({val:X(), val:Y(), val:Z(), val:W()}, ','), 'vec' end,
			 	quaternion = function( val ) return table.concat({val:X(), val:Y(), val:Z(), val:W()}, ','), 'quat' end,
			 	matrix44   = function( val )
		 			local t, v = {}, Vector4.Create()
		 			for i = 1, 4 do
		 				val:GetRow(i, v)
		 				t[#t+1] = table.concat({v:X(), v:Y(), v:Z(), v:W()}, ',')
		 			end
		 			return table.concat(t, ','), 'mtx'
		 		end,
			}

			local function toXml( tbl, indent )
				local padding = string.rep('\t', indent)
				local ipairsTbl, pairsTbl = {}, {}

				for k, v in pairs(tbl) do
					--Attempt to alleviate big nasty memory usage (50kb table was eating 1.3mb of main)
					collectgarbage('step')

					local keyNum = tonumber(k)
					local key    = keyNum and '_' or tostring(k)
					local t      = keyNum and ipairsTbl or pairsTbl

					if type(v) == 'table' then
						if tableCount(v) > 0 then
							--These big nasty bits save lots of table creates, a 180kb file can create over 1.6mb in garbage without this!
							local toStore = toXml(v, indent + tab)

							_t[#_t+1] 	= padding
							_t[#_t+1] 	= '<'
							_t[#_t+1] 	= key
							_t[#_t+1] 	= '>\n'
							_t[#_t+1] 	= toStore
							_t[#_t+1] 	= padding
							_t[#_t+1] 	= '</'
							_t[#_t+1] 	= key
							_t[#_t+1] 	= '>\n'

							t[#t+1] 	= table.concat(_t)

							local _k 	= next(_t)
							while _k do
								_t[_k] 	= nil
								_k 		= next(_t)
							end
						else
							_t[#_t+1] 	= padding
							_t[#_t+1] 	= '<'
							_t[#_t+1] 	= key
							_t[#_t+1] 	= '>'
							_t[#_t+1] 	= '</'
							_t[#_t+1] 	= key
							_t[#_t+1] 	= '>\n'

							t[#t+1] 	= table.concat(_t)

							local _k 	= next(_t)
							while _k do
								_t[_k] 	= nil
								_k 		= next(_t)
							end
						end
					else
						local val, valType = convert[string.lower(omnitype(v))](v)

						_t[#_t+1] 	= padding
						_t[#_t+1] 	= '<'
						_t[#_t+1] 	= key
						_t[#_t+1] 	= valType and table.concat(valType and {' type=\'', valType,  '\''}) or ''
						_t[#_t+1] 	= '>'
						_t[#_t+1] 	= tostring(val)
						_t[#_t+1] 	= '</'
						_t[#_t+1] 	= key
						_t[#_t+1] 	= '>\n'

						t[#t+1] 	= table.concat(_t)

						local _k 	= next(_t)
						while _k do
							_t[_k] 	= nil
							_k 		= next(_t)
						end
					end
				end

				table.sort(pairsTbl)
				return table.concat({table.concat(ipairsTbl), table.concat(pairsTbl)})
			end

			return table.concat({'<', tag, '>\n', toXml(tbl, tab), '</', tag, '>'})
		end
		
		return self
	end
)()

local ENCODE =
{
	string = function( val )
            -- Workaround for lack of Vector4 in classic lua interpreters.
            local pattern = ""^([+-]?[0-9]*%.?[0-9]+),([+-]?[0-9]*%.?[0-9]+),([+-]?[0-9]*%.?[0-9]+),([+-]?[0-9]*%.?[0-9]+)$""
            local x, y, z, w = val:match(pattern)
            -- If we found the vector pattern, process it
            if x and y and z and w then
                local components = {x, y, z, w}
                return table.concat(components, ','), 'vec'
            else
              return tostring(val)  -- Default string conversion
            end
        end,
	boolean    = function( x ) return tostring(x), 'bool' end,
	['nil']    = function( x ) return nil, 'nil' end,
	number     = function( x ) return math.floor(x) == x and string.format('%d', x) or tostring(x), 'num' end,
	vector4    = function( x ) return table.concat({x:X(), x:Y(), x:Z(), x:W()}, ','), 'vec' end,
	quaternion = function( x ) return table.concat({x:X(), x:Y(), x:Z(), x:W()}, ','), 'quat' end,
	matrix44   = function( x )
		local t = {}
		for i = 1, 4 do
			x:GetRow(i, v1)
			t[#t+1] = table.concat({v1:X(), v1:Y(), v1:Z(), v1:W()}, ',')
		end
		return table.concat(t, ','), 'mtx'
	end,
}

local function omnitype( x )
    local t = type(x)
	return (t ~= 'userdata') and t or Type(x)
end

function table_count(tbl)
    local count = 0
    for _ in pairs(tbl) do
        count = count + 1
    end
    return count
end

function Encode( tbl, indent, tab )
		local padding = string.rep(' ', indent)
		local ipairsTbl, pairsTbl = {}, {}

		for k, v in pairs(tbl) do
			local keyNum = tonumber(k)
			local key    = keyNum and '_' or tostring(k)
			local t      = keyNum and ipairsTbl or pairsTbl

			if type(v) == 'table' then
				if table_count(v) > 0 then
					t[#t+1] = table.concat({padding, '<', key, '>\n', Encode(v, indent + tab, tab), padding, '</', key, '>\n'})
				else
					t[#t+1] = table.concat({padding, '<', key, '>', '</', key, '>\n'})
				end
			else
				local val, valType = ENCODE[string.lower(omnitype(v))](v)
				t[#t+1] = table.concat({padding, '<', key, valType and table.concat(valType and {' type=\'', valType, '\''}) or '', '>', tostring(val), '</', key, '>\n'})
			end
		end

		table.sort(pairsTbl)
		return table.concat({table.concat(ipairsTbl), table.concat(pairsTbl)})
end

-------------------- Custom code for MultiServer3

PUT_CODE_HERE

-------------------- End Custom code for MultiServer3";
    }
}
