import { Component, OnInit } from '@angular/core';
import { Course } from './models';
import { HackingService } from './hacking.service';

 import 'rxjs/Rx';

@Component({
    selector: 'admin-download',
    templateUrl: 'app/admin-download.component.html',
    providers: [ HackingService ]
})
export class AdminDownloadComponent implements OnInit {
    constructor(
        private hackingService: HackingService) { }

    courses: Course[];
    selectedCourse: Course;
    exampleName: string = "";

    ngOnInit(): void {
        this.hackingService.getCourses().subscribe(data => {
            this.courses = data;
        });
    }

    onCourseChanged(obj: Course): void {
        this.selectedCourse = obj;
    }

    download(): void {
        if (this.selectedCourse != null) {
            this.hackingService.getAccessToken().subscribe(token => {
                window.open(this.hackingService.getAdminDownloadUrl(this.selectedCourse.Name, this.exampleName) + "&token=" + encodeURIComponent(token));
            });
        }
    }
}