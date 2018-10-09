import { Component, Input, OnInit } from '@angular/core';
import { User, StudentStat, StudentStatDetail, Course } from './models';
import { HackingService } from './hacking.service';




declare var jsHelper: any;

@Component({
    selector: 'admin-stats-students',
    templateUrl: 'admin-stats-students.component.html'
})
export class AdminStatsStudentsComponent implements OnInit {
    constructor(
        private hackingService: HackingService) { }

    results: StudentStat[] = [];
    results_filtered: StudentStat[] = [];

    isLoading: boolean = false;

    filteredCourse: Course;
    courses: Course[];
    showDetails: boolean = false;
    year: number = new Date().getFullYear();

    ngOnInit(): void {
        this.hackingService.getCourses().subscribe(data => {
            this.courses = data;
        });

        this.refresh();
    }

    onCourseChanged(): void {
        this.refresh();
    }

    refresh(): void {
        var self = this;
        if (!this.filteredCourse) return;

        this.isLoading = true;
        this.hackingService
            .getAdminStatsStudents(this.filteredCourse.Name, this.year || 0)
            .subscribe(data => {
                self.results = data;
                self.filter();
                self.isLoading = false;
            }, error => {
            });
    }

    filter(): void {
        this.results_filtered = this.results;
    }

    download(): void {
        this.hackingService.getAccessToken().subscribe(token => {
            window.open(this.hackingService.getAdminStatsStudentsCSVUrl(this.filteredCourse.Name, this.year) + "&token=" + encodeURIComponent(token));
        });
    }
    downloadDetails(): void {
        this.hackingService.getAccessToken().subscribe(token => {
            window.open(this.hackingService.getAdminStatsStudentDetailsCSVUrl(this.filteredCourse.Name, this.year) + "&token=" + encodeURIComponent(token));
        });
    }
}
