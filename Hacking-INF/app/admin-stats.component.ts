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
        this.hackingService.getCourses().subscribe(data => {
            this.courses = data;
        });

        this.refresh();
    }

    onCourseChanged(): void {
        this.filter();
    }

    refresh(): void {
        var self = this;
        this.isLoading = true;
        this.hackingService
            .getAdminStats()
            .subscribe(data => {
                self.results = data;
                self.filter();
                self.isLoading = false;
            }, error => {
            });
    }

    filter(): void {
        if (this.filteredCourse == null || typeof this.filteredCourse === "string") {
            this.results_filtered = this.results;
        }
        else {
            this.results_filtered = this.results.filter(i => i.Course == this.filteredCourse.Name);
        }
    }
}