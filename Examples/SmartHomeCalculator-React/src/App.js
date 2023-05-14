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
            setcanvasFacade({
              canvasFacade: {
                ...canvasFacade.canvasFacade,
                DrawingHelper: {
                  Top: rect.top,
                  Left: rect.left,
                  Width: rect.width,
                  Height: rect.height
                }
              }
            });
          }
        }, 1000)
      }, 3000);
    }


  });
  const [canvasFacade, setcanvasFacade] = useState({ canvasFacade: facade });
  function changeRadioButton(changeEvent) {
    setcanvasFacade({
      canvasFacade: {
        ...canvasFacade.canvasFacade,
        selectedDrawingStyle: parseInt(changeEvent.target.value)
      }
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
                {canvasFacade.canvasFacade.drawingTypes.map(x =>
                  <div className="radio align-content-start d-flex">
                    <label className="justify-content-center d-flex">
                      <input type="radio" key={x.value} onChange={(eventChange) => changeRadioButton(eventChange)} value={x.value} checked={x.value === canvasFacade.canvasFacade.selectedDrawingStyle} />
                      <label>&nbsp;{x.name}</label>
                    </label>
                  </div>)}
                <label className="align-content-start d-flex flex-column">
                  <label className="align-self-start">Grid density: </label>
                  <input type="number" onInput={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      gridDensity: changeEvent.target.value ?? 1
                    }
                  })} min="1" step="1" required={true} value={canvasFacade.canvasFacade.gridDensity} />
                </label>

                <label className="align-content-start d-flex">
                  <input type="checkbox" onChange={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      showGrid: !canvasFacade.canvasFacade.showGrid
                    }
                  })} checked={canvasFacade.canvasFacade.showGrid} /> &nbsp;
                  Is grid visible
                </label>

                <label className="align-content-start d-flex">
                  <input type="checkbox" onChange={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      snapToGrid: !canvasFacade.canvasFacade.snapToGrid
                    }
                  })} checked={canvasFacade.canvasFacade.snapToGrid} /> &nbsp;
                  Snap to grid
                </label>

                <label className="align-content-start d-flex flex-column">
                  <label className="align-self-start">Pixels to close room: </label>
                  <input type="number" onInput={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      autoComplete: changeEvent.target.value ?? 0
                    }
                  })} min="0" step="0.25" required={true} value={canvasFacade.autoComplete} />
                </label>

                <label className="align-content-start d-flex flex-column">
                  <label className="align-self-start">Line thickness: </label>
                  <input type="number" onInput={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      thickness: changeEvent.target.value ?? 1
                    }
                  })} min="1" step="0.25" required={true} value={canvasFacade.canvasFacade.thickness} />
                </label>

                <label className="justify-content-start d-flex">
                  <input type="checkbox" onChange={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      deleteMode: !canvasFacade.canvasFacade.deleteMode
                    }
                  })} checked={canvasFacade.canvasFacade.deleteMode} /> &nbsp;
                  Delete Mode
                </label>

                <label className="align-items-start d-flex flex-column">
                  <label className="align-self-start">Delete Range: </label>
                  <input type="number" onInput={changeEvent => setcanvasFacade({
                    canvasFacade: {
                      ...canvasFacade.canvasFacade,
                      deleteRange: changeEvent.target.value ?? 0
                    }
                  })} min="0" step="0.25" required={true} value={canvasFacade.canvasFacade.deleteRange} />
                </label>
              </div>
            </div>
          ) : null
          }
          <div className="w-75 mr-1">
            {shouldRenderCanvas ? <CanvasContainer facadeWrapper={canvasFacade} facadeWrapperChanged={wrapper => setcanvasFacade(wrapper)} /> : null}
          </div>


          <div className="w-25 mr-1 fs-5">
            <h5 className="justify-self-center align-self-center">Selected devices:</h5>
            {
              canvasFacade.canvasFacade.devicePrices.map(x =>
                <div className="d-flex flex-row w-100">
                  <label className="flex-fill w-0">{x.count + "X"}</label>
                  <label className="flex-fill w-0">{x.name}</label>
                  <label className="flex-fill w-0 d-flex justify-content-end">{x.price + "$"}</label>
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
