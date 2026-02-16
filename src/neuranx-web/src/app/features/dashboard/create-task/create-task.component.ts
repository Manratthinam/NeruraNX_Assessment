import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
  ],
  templateUrl: './create-task.component.html',
  styleUrls: ['./create-task.component.css'],
})
export class CreateTaskComponent implements OnInit {
  taskId: string | null = null;
  isUpdateMode = false;
  title: string = 'Create Task';

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.authService.getUsers().subscribe();
    this.route.data.subscribe((data) => {
      if (data['title']) {
        this.title = data['title'];
      }
    });

    if (this.title === 'Update Task') {
      this.taskId = this.route.snapshot.paramMap.get('id');
      this.loadTaskDetails();
    }
  }

  private fb = inject(FormBuilder);
  public authService = inject(AuthService);
  private taskService = inject(TaskService);
  private router = inject(Router);

  createTaskForm = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    status: ['Pending'],
    assignedToUserId: [''],
  });

  loadTaskDetails() {
    this.taskService.getTaskById(Number(this.taskId)).subscribe({
      next: (res) => {
        const { title, description, status, assignedToUserId } = res.tasks;
        this.createTaskForm.patchValue({
          title: title,
          description: description,
          status: status,
          assignedToUserId: assignedToUserId,
        });
      },
    });
  }

  onSubmit() {
    if (this.createTaskForm.valid) {
      if (this.title === 'Update Task') {
        this.taskService.updateTask(Number(this.taskId), this.createTaskForm.value).subscribe({
          next: () => {
            this.router.navigate(['/dashboard/my-tasks']);
          },
          error: (err) => {
            console.error('Error updating task', err);
          },
        });
      } else {
        this.taskService.createTask(this.createTaskForm.value).subscribe({
          next: () => {
            this.router.navigate(['/dashboard/my-tasks']);
          },
          error: (err) => {
            console.error('Error creating task', err);
          },
        });
      }
    }
  }
}
