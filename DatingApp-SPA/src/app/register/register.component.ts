import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  @Output() registerCancelled = new EventEmitter();
  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
  }

  public register(): void {
    this.authService.register(this.model).subscribe(
      () => {
        console.log('Register successfully');
      },
      error => {
        console.error(error);
      }
    );
  }

  public cancel(): void {
    this.registerCancelled.emit(false);
    console.log('cancelled');
  }
}
