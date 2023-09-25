# Changelog
All notable changes to the Ventuz Extension API will be documented in this file.

| Version_BuildNumber_Revision | API Version | Date |
## [6.12.0x_yyy_zzzzz] [1.0.0] - dd-mm-yyyy
## [7.00.0x_yyy_zzzzz] [1.0.0] - dd-mm-yyyy
### Fixed
### Added
- Support for derived Vx-Nodes
- Support for Vx-Property Group
- VxToolBoxAttribute has now also Sort and BreakAfter properties
- new VxCategoriesAttribute for sorting categories and define their collapsed states
### Changed
- Access to API functions moved to static class VX (no API as inherited members anymore)
- The usages are now adjusted for all from VxMetaDataAttribute derived classes
- Vx methods starting with "Method" receive the event argument via VX.Node.MethodArg because they can declare properties as parameters now for easy access
- The argument for VxCategoryAttribute have changed. Use also the new VxCategoriesAttribute.


## [6.12.01_309_37961] [0.9.1] - 06-12-2022
### Fixed
- Name clashes caused by the same VX assembly prevented Ventuz from starting
- VX Matrix array conversion to the native Matrix array type did not work
- When renaming Methods old version was still displayed after deserialization (issue #27)
- VxLegacyNames attribute did not work at all
- Attributes like VxCategory or VxLegacyNames were not applied to Node methods 
### Added
- Color4b arrays are now a supported property type 
- Static VxLog class for simpler logging to Ventuz logs 
### Changed
- ParameterSets for ICustomResource instances must now implement Equals() & GetHashCode()

## [6.12.00_242_37316] [0.9.0] - 26-10-2022
### Fixed
Vx: fixed issues with Node validation, which was caused by incorrect dependencies of in- and output properties

All your extensions must be recompiled!

## [6.12.00_219_37118] [0.9.0] - 11-10-2022
### Fixed
- vx.exe: wrong PlatformTarget in Release configuration - now also x64
### Added
- Vx: base class VxNode has now a method to invalidate the node by a given property or completely
### Changed

