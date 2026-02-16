import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Task {
    id: number;
    title: string;
    description: string;
    status: string;
    assignee: string | null;
}

export interface TaskList {
    tasks: Task;
    name: string;
}

export interface PaginatedResult<T> {
    items: T[];
    pageIndex: number;
    totalPages: number;
    totalCount: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    private apiUrl = 'http://localhost:5000/api/Task';

    constructor(private http: HttpClient) { }


    getTasks(pageIndex: number, pageSize: number, user: string): Observable<PaginatedResult<TaskList>> {
        let params = new HttpParams()
            .set('pageNumber', pageIndex.toString())
            .set('pageSize', pageSize.toString())
            .set('user', user);

        return this.http.get<PaginatedResult<TaskList>>(`${this.apiUrl}/GetTasks`, { params });
    }

    createTask(task: any): Observable<Task> {
        return this.http.post<Task>(`${this.apiUrl}`, task);
    }

    getTaskById(Id: number): Observable<any> {

        return this.http.get<any>(`${this.apiUrl}/${Id}`);
    }

    updateTask(Id: number, task: any): Observable<Task> {
        return this.http.put<Task>(`${this.apiUrl}/${Id}`, task);
    }

    deleteTask(Id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${Id}`);
    }


}
