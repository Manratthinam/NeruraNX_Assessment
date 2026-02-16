import { Component, signal, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { AuthService } from '../../../core/services/auth.service';
import { LoaderComponent } from '../../loader.component';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterModule,
        MatCardModule,
        MatInputModule,
        MatButtonModule,
        MatFormFieldModule,
        LoaderComponent
    ],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    loginForm: FormGroup;
    error = signal<string>('');
    loader = signal<boolean>(false);

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.loginForm = this.fb.group({
            Email: ['', [Validators.required, Validators.email]],
            Password: ['', [Validators.required]]
        });
    }

    onSubmit() {
        if (this.loginForm.valid) {
            this.loader.update(v => !v);
            this.authService.login(this.loginForm.value).subscribe({
                next: () => {
                    console.log('Login successful');
                    this.loader.update(v => !v);
                    this.router.navigate(['/dashboard']);
                },
                error: (err: any) => {
                    this.error.set('Invalid email or password');
                    this.loader.update(v => !v);
                    console.error(err);
                }
            });
        }
    }
}
