# NBK Extensions - Visual Studio Extensions for Visual studio 2022

## Overview
The NBK Extensions Visual Studio Extension is designed to enhance the development experience within Visual Studio 2022. It provides functionality related to managing project references conveniently from within the IDE.

## Features
- **Load Multiple Projects:** Loads multiple projects available in sub folders into a solution. This also provides option to set internal references after loading projects.
- **Set Internal References:** Replaces external references of projects added to the solution with references to internal projects within the solution, if available.
- **Set Local References:** [Not yet implemented] This feature is planned to allow users to set local references within the solution. Stay tuned for updates!

### Load Multiple Projects
> NBK Extensions > Load Multiple Projects

![image](https://media.github.cerner.com/user/13407/files/9a112e15-a110-4ea7-83c8-7eba58d36a52)
![image](https://media.github.cerner.com/user/13407/files/012c8f2f-7be6-4e63-a512-663809c3b141)

#### Usage
1. After installing the extension, open Visual Studio 2022.
2. Navigate to the "Extensions" menu and then to the "NBK Extensions" menu.
3. Select "Load Multiple Projects" to open projects selection window.
4. Click magnifier icon in "Select folder to scan" to open folder selection and select the parent folder with multiple projects (.csproj)
5. select the checkboxes for the projects that needs to be loaded into the solution 
6. Check 'Set internal references after load' if the projects references need to be modified.

### Internal Reference Tool
> NBK Extensions > Set Internal References

![image](https://media.github.cerner.com/user/13407/files/161a8959-cfad-41ca-91df-b4da40e6b177)


#### Usage
1. After installing the extension, open Visual Studio 2022.
2. Load all the required projects into the solution.
3. Navigate to the "Extensions" menu and then to the "NBK Extensions" menu.
4. Select "Set Internal References" to replace external references with internal ones. 

## Installation
To install the NBK Extensions Visual Studio Extension, follow these steps:
1. Close all Visual studio instance
2. Download the [VSIX file](/VSIX/NBKVSExtension.vsix) from the VSIX folder  and run it to install.

**OR**

3. You can compile the source code with visual studio 2022 and install from the generated VSIX file.

## Contributing
Contributions to enhance the NBK Extensions Visual Studio Extension are welcome! If you're interested in contributing, follow these steps:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes.
4. Test thoroughly.
5. Submit a pull request.

## Contact
For any inquiries, suggestions or requests, you can contact at nishant.sukrutha@gmail.com.
