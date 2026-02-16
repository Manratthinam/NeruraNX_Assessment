import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { MatProgressSpinner } from "@angular/material/progress-spinner";

@Component({
    selector: 'app-loader',
    standalone: true,
    imports: [CommonModule, MatProgressSpinner],
    templateUrl: './loader.component.html',
    styleUrls: ['./loader.component.css']
})

export class LoaderComponent { 

}