Publishing the application:
1) Have .net 7 sdk installed
2) Navigate to either Examples/SmartHomeCalculator or to Examples/SmartHomeCalculator-Wasm
3) In terminal run command dotnet publish -c Release --sc --runtime <Targeted platform>(https://learn.microsoft.com/en-us/dotnet/core/rid-catalog) optionaly you can add -o and output directory


For running this application you have several options.

For windows:
1) Navigate to Lib/Blazor/Server and run SmartHomeCalculator.exe - server will run with ip address http://localhost:5000
2) Set up an IIS with URL Rewrite module instaled in either Lib/Blazor/Server or Lib/Blazor/Wasm

For any platform:
1) For BlazorServer: Publish the app acording to the steps above then navigate to the output directory and in terminal run dotnet SmartHomeCalculator.dll 
3) Have .net 7 SDK and run dotnet watch command in either Examples/SmartHomeCalculator or Examples/SmartHomeCalculator-Wasm - application will run on http://localhost:5000 and http://localhost:5001
4) Same as 3 for SmartHomeCalculator-Wasm but the go to the Examples/SmartHomeCalculator-React and run on either yarn or npm commands install and start - react application can be find on http://localhost:3000
5) Open the SmartHomeCalculator.sln in .net IDE(i.e. Visual Studio or Rider) and build and run either SmartHomeCalculator project or 
SmartHomeCalculator-Wasm project