import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
    private authService = inject(AuthService);
    private router = inject(Router);

    showTimeoutModal = signal<boolean>(false);
    private timeoutId: any;
    private readonly TIMEOUT_DURATION = 13 * 60 * 1000; // 1 minutes

    ngOnInit() {
        this.resetTimer();
    }

    ngOnDestroy() {
        clearTimeout(this.timeoutId);
    }

    resetTimer() {
        if (this.showTimeoutModal()) return; // Don't reset if modal is showing

        clearTimeout(this.timeoutId);
        this.timeoutId = setTimeout(() => {
            this.showTimeoutModal.update(v => true);
        }, this.TIMEOUT_DURATION);
    }

    continueSession() {
        this.authService.refreshTokenCall().subscribe({
            next: () => {
                this.showTimeoutModal.update(v => false);
                this.resetTimer();
            },
            error: () => this.logout()
        });
    }

    logout() {
        this.authService.logout();
        this.router.navigate(['/auth/login']);
    }
}
