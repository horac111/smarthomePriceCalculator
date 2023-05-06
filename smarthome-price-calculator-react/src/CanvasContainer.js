import { useBlazor } from './blazor-react';

export function CanvasContainer({
  project,
}) {
  const fragment = useBlazor('canvas-container', {
    project,
  });

  return fragment;
}
