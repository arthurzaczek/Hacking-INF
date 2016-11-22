﻿import { Component, OnInit } from '@angular/core';
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

    getCourses(): void {
        this.hackingService.getCourses().then(data => {
            this.courses = data;
            console.log("CoursesComponent: got data from service");
        });
        console.log("CoursesComponent.getCourses");
    }

    ngOnInit(): void {
        this.getCourses();
    }
}