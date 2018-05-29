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
        this.refresh();
        let self = this;
        this.hackingService
            .userLoginEvent
            .subscribe(item => {
                console.log("User changed, refreshing courses");
                self.refresh();
            });
    }

    refresh(): void {
        this.hackingService.getCourses().subscribe(data => {
            this.courses = data;
        });
    }
}
