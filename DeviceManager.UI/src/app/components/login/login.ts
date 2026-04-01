import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { LoginRequest } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  data: LoginRequest = {
    email: '',
    password: ''
  };

  submitted = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {
    this.submitted = true;
    this.errorMessage = '';

    if (!this.data.email || !this.data.password) {
      return;
    }

    this.authService.login(this.data).subscribe({
      next: () => this.router.navigate(['/devices']),
      error: (err) => {
        this.errorMessage = err.error?.message || 'Login failed.';
      }
    });
  }
}