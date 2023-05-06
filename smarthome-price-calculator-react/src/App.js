import logo from './logo.svg';
import './App.css';
import './CanvasContainer'
import { CanvasContainer } from './CanvasContainer';
import React, { useCallback, useState } from 'react';
import project from './project.json'

function App() {
  console.log(JSON.stringify(project));
  const [nextCounterIndex, setNextCounterIndex] = useState(1);
  const [blazorCounters, setBlazorCounters] = useState([]);
  const addBlazorCounter = () => {
    const index = nextCounterIndex;
    setNextCounterIndex(index + 1);
    setBlazorCounters(blazorCounters.concat([{
      title: `ahoj ${index}`,
      project: project
    }]));
  };
  const removeBlazorCounter = () => {
    setBlazorCounters(blazorCounters.slice(0, -1));
  };
  return (
    <div className="App">
      <header className="App-header">
      <img src={logo} className="App-logo" alt="logo" />
        <p>
          <button onClick={addBlazorCounter}>Add Blazor counter</button> &nbsp;
          <button onClick={removeBlazorCounter}>Remove Blazor counter</button> &nbsp;
        </p>

        {blazorCounters.map(counter =>
          <div key={counter.title}>
            <CanvasContainer project={ JSON.stringify(counter.project)}>
            </CanvasContainer>
          </div>
        )}
      </header>
    </div>
  );
}

export default App;
