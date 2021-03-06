import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  // username: michele
  // password: password

  constructor(
    private authService: AuthService  ) { }

  ngOnInit(): void {
  }

  login(): void {
    this.authService.login(this.model)
      .subscribe(next => {
        console.log('login successfully');
      },
      error => {
        console.log(error);
      });
  }


  loggedIn(): boolean{
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout(): void {
    localStorage.removeItem('token');
    console.log('logged out');
  }
}
