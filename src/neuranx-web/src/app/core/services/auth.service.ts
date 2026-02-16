import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'http://localhost:5000/api/auth';
    private tokenKey = 'authToken';
    private refreshToken = 'refreshToken';
    private userKey = 'authUser';
    
    public users$ = new BehaviorSubject<any>(null);
    private currentUserSubject = new BehaviorSubject<any>(null);
    currentUser$ = this.currentUserSubject.asObservable();

    constructor(private http: HttpClient) {
        this.loadUserFromStorage();
    }

    private loadUserFromStorage() {
        const token = localStorage.getItem(this.tokenKey);
        // In a real app, you'd decode the token or fetch user details here
        if (token) {
            this.currentUserSubject.next({ token }); // Simplified
        }
    }

    register(user: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/register`, { payload: user });
    }

    login(credentials: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/login`, { payload: credentials }).pipe(
            tap(response => {
                const { success, token } = response;
                if (success && token) {
                    this.setSession(response);
                }
            })
        );
    }

    private setSession(authResult: any) {
        localStorage.setItem(this.tokenKey, authResult.token);
        localStorage.setItem(this.refreshToken, authResult.refreshToken);
        this.currentUserSubject.next({ token: authResult.token });
    }

    refreshTokenCall(): Observable<any> {
        const refreshToken = localStorage.getItem(this.refreshToken);
        const token = localStorage.getItem(this.tokenKey);
        return this.http.post<any>(`${this.apiUrl}/refresh-token`, { Token: token, RefreshToken: refreshToken }).pipe(
            tap(response => {
                const { success, token } = response;
                if (success && token) {
                    this.setSession(response);
                }
            })
        );
    }

     getUsers(): Observable<string[]> {
        return this.http.get<string[]>(`${this.apiUrl}/GetAllUsers`).pipe(
            tap(users => {
                this.users$.next(users);
            })
        );
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        this.currentUserSubject.next(null);
    }

    getToken(): string | null {
        return localStorage.getItem(this.tokenKey);
    }

    isAuthenticated(): boolean {
        return !!this.getToken();
    }
}
