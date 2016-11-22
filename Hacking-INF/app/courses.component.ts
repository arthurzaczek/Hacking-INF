import { Component, OnInit } from '@angular/core';
import { Course } from './models';
import { HackingService } from './hacking.service';

@Component({
    selector: 'list-courses',
    templateUrl: 'app/courses.component.html',
    providers: [ HackingService ]
})
export class CoursesComponent implements OnInit {
    constructor(private hackingService: HackingService) { }

    courses: Course[];

    ngOnInit(): void {
        this.hackingService.getCourses().then(data => {
            this.courses = data;
        });
    }
}