For running this application you have several options.

1) Navigate to ./Lib/Blazor/Server and run SmartHomeCalculator.exe - server will run with ip address http://localhost:5000
2) Set up an IIS with URL Rewrite module instaled in either ./Lib/Blazor/Server or ./Lib/Blazor/Wasm
3) Have .net 7 SDK and run dotnet watch command in either ./Examples/SmartHomeCalculator or ./Examples/SmartHomeCalculator-Wasm - application will run on http://localhost:5000 and http://localhost:5001
4) Same as 3 for SmartHomeCalculator-Wasm but the go to the ./Examples/SmartHomeCalculator-React and run on either yarn or npm commands install and start - react application can be find on http://localhost:3000
5) Open the ./SmartHomeCalculator.sln in .net IDE(i.e. Visual Studio or Rider) and build and run either SmartHomeCalculator project or 
SmartHomeCalculator-Wasm project