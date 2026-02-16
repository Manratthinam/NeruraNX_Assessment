import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';

export const DASHBOARD_ROUTES: Routes = [
    {
        path: '',
        component: DashboardComponent,
        children: [
            {
                path: 'my-tasks',
                loadComponent: () => import('./task-list/task-list.component').then(m => m.TaskListComponent),
                data: { title: 'My Tasks' }
            },
            {
                path: 'all-tasks',
                loadComponent: () => import('./task-list/task-list.component').then(m => m.TaskListComponent),
                data: { title: 'All Tasks' }
            },
            {
                path: 'create-task',
                loadComponent: () => import('./create-task/create-task.component').then(m => m.CreateTaskComponent),
                data: { title: 'Create Task' }
            },
             {
                path: 'update-task/:id',
                loadComponent: () => import('./create-task/create-task.component').then(m => m.CreateTaskComponent),
                data: { title: 'Update Task' }
            },
            {
                path: '',
                redirectTo: 'my-tasks',
                pathMatch: 'full'
            }
        ]
    }
];
