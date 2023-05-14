import { useBlazor } from './blazor-react';

export function CanvasContainer({
  facadeWrapper, facadeWrapperChanged,
}) {
  const fragment = useBlazor('canvas-container', {
    facadeWrapper, facadeWrapperChanged,
  });

  return fragment;
}
