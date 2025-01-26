import { Component, OnInit, inject, input, output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { HomeComponent } from '../home/home.component';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule, JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,HomeComponent ,CommonModule,JsonPipe,NgIf,TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
 
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  cancelRegister = output<boolean>();
  private router = inject(Router);
  model: any = {}

  registrationForm: FormGroup = new FormGroup({});
  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registrationForm = new FormGroup({
      username: new FormControl('', Validators.required),
      gender: new FormControl('', [Validators.required,this.validateGender]),
      knownAs: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4),
      Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
    });
    this.registrationForm.controls['password'].valueChanges.subscribe({
      next:()=>this.registrationForm.controls['confirmPassword'].updateValueAndValidity()
    })

  }

  matchValues(matchTo: string): ValidatorFn {

    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {ismatching:true}
    }

  }

  validateGender(): ValidatorFn {
    return (control: AbstractControl) => {
      const value = control.value?.toLowerCase();
      return value === 'male' || value === 'female'
        ? null
        : { invalidGender: true }; // Error key if validation fails
    };
  }

  register() {
    if (this.registrationForm.valid) {
      this.accountService.register(this.registrationForm.value).subscribe({
        next: _ => this.router.navigateByUrl('/members'),
        error: error => this.toastr.error(error.error)
      });
    } else {
      this.registrationForm.markAllAsTouched();
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
