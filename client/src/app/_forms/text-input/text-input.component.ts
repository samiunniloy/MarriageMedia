import { CommonModule, NgIf } from '@angular/common';
import { Component, Optional, Self, input } from '@angular/core';
import { ControlValueAccessor, FormControl,FormGroup, FormsModule, NG_VALUE_ACCESSOR, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  standalone:true,
  imports: [NgIf, ReactiveFormsModule, CommonModule, FormsModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    multi: true,
    useExisting: TextInputComponent
  }],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.css'
})
export class TextInputComponent implements ControlValueAccessor {


  label = input<string>('');
  type = input<string>('text');
  constructor(
  //  @Optional() // Add this decorator
    @Self()
    public ngControl: NgControl
  ) {
    // Better to add some checks here since controlDir can be null
    if (ngControl) {
      ngControl.valueAccessor = this;
    }
  }

    writeValue(obj: any): void {
      
    }
    registerOnChange(fn: any): void {
        
    }
    registerOnTouched(fn: any): void {
      
    }
  get control(): FormControl {
    return this.ngControl?.control as FormControl
  }
}
