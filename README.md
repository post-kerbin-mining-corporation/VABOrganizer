# VAB Organizer

A mod for Kerbal Space Program, intended to modify how the parts list is managed and sorted.

* [Features](#features)
* [Dependencies](#dependencies)
* [Installation](#installation)
* [Contributing](#contributing)
* [License](#licensing)

## Features

This mod attempts to allow the parts list to mimic the hierarchical model that I have seen used by many people in the community. Often people look for parts by...
1. Looking for a function (e.g. an engine) then....
2. Looking for a subfunction (e.g. a solid rocket engine) then... 
3. Looking for the appropriate cross sectional aka bulkhead size. 
4. Then compare other things like mass, etc. 

There are therefore 3 core features of the mod.

### Bulkhead Tags
Bulkhead size tags are a new feature which appends a colored tag to parts according to their bulkhead size. This lets the player quickly see how big a part is in the parts list and intuitively where it will fit in their current vessel. 

### Bulkhead Sorting
The mod also adds a bulkhead size sort mode, which is enabled by default in the VAB. This sorts parts by bulkhead size in ascending or descending order. 

To make this fit in limited UI space, the VAB sorters have been modified so they show icons instead of text descriptors.

### Subcategories

Subcategories are new VAB parts list widgets that appear as headers within the Function category lists. They can be collapsed or expanded, and sort parts in these categories more easily. As an example, parts in **Command** may be sorted into **Pods**, **Probes**, **Cockpits** and similar. 

## Dependencies

### Required
These components are required for the mod to function and are bundled as part of any download:
* [HarmonyKSP](https://github.com/KSPModdingLibs/HarmonyKSP)
* [Module Manager]((https://github.com/sarbian/ModuleManager))

## Installation

To install, place the GameData folder inside your Kerbal Space Program folder. If asked to overwrite files, please do so.

NOTE: Do NOT rename or move folders within the GameData folder - this mod uses absolute paths to assets and will break if this happens.

## Contributing

I certainly accept pull requests. Please target all such things to the `dev` branch though!

## Licensing

MIT license:

Copyright (c) 2024 Chris Adderley
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


Any bundled mods are distributed under their own licenses:
* ModuleManager by ialdabaoth and sarbian is distributed under a Creative Commons Sharealike license. More details, including source code, can be found [here](http://forum.kerbalspaceprogram.com/threads/31342-0-20-ModuleManager-1-3-for-all-your-stock-modding-

* HarmonyKSP is distributed under the MIT License (see https://github.com/KSPModdingLibs/HarmonyKSP/blob/main/LICENSE)