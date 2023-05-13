import './App.css';
import './CanvasContainer'
import { CanvasContainer } from './CanvasContainer';
import React, { useEffect, useState } from 'react';
import facade from './facade.json';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faArrowRight } from '@fortawesome/free-solid-svg-icons';

function App() {
  const [shouldRenderCanvas, setShouldRenderCanvas] = useState(false);
  const [showSidebar, setShowSidebar] = useState(true);
  useEffect(() => {
    if (!shouldRenderCanvas) {
      setTimeout(() => {
        setShouldRenderCanvas(true);
        setTimeout(() => {
          const canvasContainer = document.querySelector("#CanvasContainer");
          if (canvasContainer) {
            const rect = canvasContainer.getBoundingClientRect();
            setCanvasFacade({
              ...canvasFacade,
              DrawingHelper: {
                Top: rect.top,
                Left: rect.left,
                Width: rect.width,
                Height: rect.height
              }
            });
          }
        }, 1000)
      }, 3000);
    }


  });
  const [canvasFacade, setCanvasFacade] = useState(facade);
  console.log(canvasFacade);
  function changeRadioButton(changeEvent) {
    setCanvasFacade({
      ...canvasFacade,
      SelectedDrawingStyle: parseInt(changeEvent.target.value)
    })
  }
  return (
    <div className="App" >
      <header className="App-header h-100 w-100 main ml-5 mr-5">
        <div className="d-flex flex-grow-1 h-100 w-100">
          <button className="z-2 m-2 position-absolute" onClick={() => setShowSidebar(!showSidebar)}>
            <FontAwesomeIcon icon={showSidebar ? faArrowLeft : faArrowRight} />
          </button>
          {showSidebar ? (
            <div className="w-25 z-1 pl-3 position-absolute bg-warning">
              <div className="mt-5 m-2 d-flex flex-column">
                {canvasFacade.DrawingTypes.map(x =>
                  <div className="radio align-content-start d-flex">
                    <label className="justify-content-center d-flex">
                      <input type="radio" key={x.Value} onChange={(eventChange) => changeRadioButton(eventChange)} value={x.Value} checked={x.Value === canvasFacade.SelectedDrawingStyle} />
                      <label>&nbsp;{x.Name}</label>
                    </label>
                  </div>)}
                <label className="align-content-start d-flex flex-column">
                  <label className="align-self-start">Grid density: </label>
                  <input type="number" onInput={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    GridDensity: changeEvent.target.value ?? 1
                  })} min="1" step="1" required={true} value={canvasFacade.GridDensity} />
                </label>

                <label className="align-content-start d-flex">
                  <input type="checkbox" onChange={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    ShowGrid: !canvasFacade.ShowGrid
                  })} checked={canvasFacade.ShowGrid} /> &nbsp;
                  Is grid visible
                </label>

                <label className="align-content-start d-flex">
                  <input type="checkbox" onChange={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    SnapToGrid: !canvasFacade.SnapToGrid
                  })} checked={canvasFacade.SnapToGrid} /> &nbsp;
                  Snap to grid
                </label>

                <label className="align-content-start d-flex flex-column">
                  <label className="align-self-start">Pixels to close room: </label>
                  <input type="number" onInput={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    AutoComplete: changeEvent.target.value ?? 0
                  })} min="0" step="0.25" required={true} value={canvasFacade.AutoComplete} />
                </label>

                <label className="align-content-start d-flex flex-column">
                  <label className="align-self-start">Line thickness: </label>
                  <input type="number" onInput={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    Thickness: changeEvent.target.value ?? 1
                  })} min="1" step="0.25" required={true} value={canvasFacade.Thickness} />
                </label>

                <label className="justify-content-start d-flex">
                  <input type="checkbox" onChange={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    DeleteMode: !canvasFacade.DeleteMode
                  })} checked={canvasFacade.DeleteMode} /> &nbsp;
                  Delete Mode
                </label>

                <label className="align-items-start d-flex flex-column">
                  <label className="align-self-start">Delete Range: </label>
                  <input type="number" onInput={changeEvent => setCanvasFacade({
                    ...canvasFacade,
                    DeleteRange: changeEvent.target.value ?? 0
                  })} min="0" step="0.25" required={true} value={canvasFacade.DeleteRange} />
                </label>
              </div>
            </div>
          ) : null
          }
          <div className="w-75 mr-1">
            {shouldRenderCanvas ? <CanvasContainer facadeWrapper={{ CanvasFacade: canvasFacade }} /> : null}
          </div>


          <div className="w-25 mr-1">
            <h5 className="justify-self-center align-self-center">Selected devices:</h5>
            {
              canvasFacade.DevicePrices.map(x =>
                <div className="d-flex flex-row w-100">
                  <label className="flex-fill w-0">{x.Count + "X"}</label>
                  <label className="flex-fill w-0">{x.Name}</label>
                  <label className="flex-fill w-0 d-flex justify-content-end">{x.Price + "$"}</label>
                </div>
              )

            }
          </div>
        </div>
      </header>
    </div>


  );
}

export default App;
