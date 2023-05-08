import logo from './logo.svg';
import './App.css';
import './CanvasContainer'
import { CanvasContainer } from './CanvasContainer';
import React, { useCallback, useEffect, useState } from 'react';
import project from './project.json'

function App() {
  const [shouldRenderCanvas, setShouldRenderCanvas] = useState(false);
  useEffect(() => { setTimeout(() => setShouldRenderCanvas(true), 5000);});
  return (
    <div className="App">
      <header className="App-header">
           { shouldRenderCanvas ? <CanvasContainer project={ JSON.stringify(project)} /> : null}
      </header>
    </div>
  );
}

export default App;
