import { Component, OnInit, inject, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HomeComponent } from '../home/home.component';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,HomeComponent ,JsonPipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
 
  private accountService = inject(AccountService);
  private toastr=inject(ToastrService)
  cancelRegister = output<boolean>();
  model: any = {}

  registrationForm: FormGroup = new FormGroup({});
  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registrationForm = new FormGroup({
      username: new FormControl('hello',Validators.required),
      password: new FormControl('',[Validators.required, Validators.minLength(4),
      Validators.maxLength(8)]),
      confirmPassword:new FormControl('',[Validators.required]),
    })
  }

  register() {
    console.log(this.registrationForm.value);
    //this.accountService.register(this.model).subscribe({
    //  next: response => {
    //    console.log(response);
    //    this.cancel();
    //  },
    //  error: error => this.toastr.error(error.error)
    //})
  }
  cancel() {
    this.cancelRegister.emit(false);
  }

}
