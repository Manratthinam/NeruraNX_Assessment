import { Component, OnInit, inject, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { TaskService, Task } from '../../../core/services/task.service';
import { map, tap } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-task-list',
    standalone: true,
    imports: [CommonModule, MatTableModule, MatPaginatorModule, RouterLink, MatButtonModule, MatIconModule],
    templateUrl: './task-list.component.html',
    styleUrls: ['./task-list.css']
})
export class TaskListComponent implements OnInit, AfterViewInit {
    title: string = 'Tasks';
    private route = inject(ActivatedRoute);
    private taskService = inject(TaskService);

    displayedColumns: string[] = ['id', 'title', 'description', 'status', 'assignee', 'actions']; // Added actions
    dataSource = new MatTableDataSource<Task>([]);
    totalCount = 0;

    @ViewChild(MatPaginator) paginator!: MatPaginator;

    ngOnInit() {
        this.route.data.subscribe(data => {
            if (data['title']) {
                this.title = data['title'];
            }
        });
    }

    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.paginator.page
            .pipe(
                tap(() => this.loadTasks())
            )
            .subscribe();

        this.loadTasks();
    }

    loadTasks() {
        if (this.title === 'All Tasks') {
            this.taskService.getTasks(this.paginator.pageIndex + 1, this.paginator.pageSize, "All")
                .subscribe({
                    next: (res) => {
                        const transformedData = res.items.map(item => ({
                            id: item.tasks.id,
                            title: item.tasks.title,
                            description: item.tasks.description,
                            status: item.tasks.status,
                            assignee: item.name
                        }));
                        this.dataSource.data = transformedData;
                        this.totalCount = res.totalCount;
                    },
                    error: (err) => {
                        console.error('Error loading tasks', err);
                    }
                });
        }
        else {
            this.taskService.getTasks(this.paginator.pageIndex + 1, this.paginator.pageSize, "My")
                .subscribe({
                    next: (res) => {
                        const transformedData = res.items.map(item => ({
                            id: item.tasks.id,
                            title: item.tasks.title,
                            description: item.tasks.description,
                            status: item.tasks.status,
                            assignee: item.name
                        }));
                        this.dataSource.data = transformedData;
                        this.totalCount = res.totalCount;
                    },
                    error: (err) => {
                        console.error('Error loading tasks', err);
                    }
                });
        }
    }

    deleteTask(id: number) {
        if (confirm('Are you sure you want to delete this task?')) {
            this.taskService.deleteTask(id).subscribe({
                next: () => {
                    this.loadTasks(); // Refresh the list
                },
                error: (err) => {
                    console.error('Error deleting task', err);
                }
            });
        }
    }
}

