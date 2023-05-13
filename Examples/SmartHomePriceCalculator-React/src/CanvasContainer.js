import { useBlazor } from './blazor-react';

export function CanvasContainer({
  facadeWrapper,
}) {
  const fragment = useBlazor('canvas-container', {
    facadeWrapper,
  });

  return fragment;
}
