import { Component, Input, OnInit } from '@angular/core';
import { User, ExampleStat, Course } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/groupBy';

declare var jsHelper: any;

@Component({
    selector: 'admin-stats',
    templateUrl: 'app/admin-stats.component.html'
})
export class AdminStatsComponent implements OnInit {
    constructor(
        private hackingService: HackingService) { }

    results: ExampleStat[] = [];
    results_filtered: ExampleStat[] = [];

    isLoading: boolean = true;

    filteredCourse: Course;
    courses: Course[];

    ngOnInit(): void {
        var self = this;
        this.hackingService.getCourses().subscribe(data => {
            this.courses = data;
        });
        this.hackingService
            .getAdminStats()
            .subscribe(data => {
                self.results = data;
                self.results_filtered = data;
                self.filteredCourse = null;
                self.isLoading = false;
            }, error => {
            });
    }

    onCourseChanged(obj: Course): void {
        this.filteredCourse = obj;
        if (obj == null || typeof obj === "string") {
            this.results_filtered = this.results;
        }
        else {
            this.results_filtered = this.results.filter(i => i.Course == obj.Name);
        }
    }
}