import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { RegisterRequest } from '../../models/auth.model';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  data: RegisterRequest = {
    name: '',
    email: '',
    password: '',
    role: '',
    location: ''
  };

  submitted = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register(): void {
    this.submitted = true;
    this.errorMessage = '';

    if (!this.data.name || !this.data.email || !this.data.password ||
        !this.data.role || !this.data.location) {
      return;
    }

    this.authService.register(this.data).subscribe({
      next: () => this.router.navigate(['/login']),
      error: (err) => {
        this.errorMessage = err.error?.message || 'Registration failed.';
      }
    });
  }
}