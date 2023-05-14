import {{ Component, EventEmitter, Input, Output }} from '@angular/core';
import {{ BlazorAdapterComponent }} from '../blazor-adapter/blazor-adapter.component';

@Component({{
  selector: '{canvas-container}',
  template: '',
}})

export class CanvasContainerComponent extends BlazorAdapterComponent {{
  @Input() canvasFacade: object | null = null;
  @Output() facadeWrapperChanged: EventEmitter<any> = new EventEmitter();
}}