import { Component, OnInit } from '@angular/core';
import { Course } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/Rx';
declare var jsHelper: any;

@Component({
    selector: 'admin-loadtest',
    templateUrl: 'app/admin-loadtest.component.html'
})
export class AdminLoadTestComponent {
    constructor(
        private hackingService: HackingService) { }

    result: string = "";
    isError: boolean = false;
    concurrent: number = 70;
    numberOfTests: number = 10;

    loadTest(): void {
        var self = this;
        this.result = "";
        this.isError = false;
        jsHelper.showWaitDialog();
        this.hackingService.loadTest(this.concurrent, this.numberOfTests).subscribe(result => {
            self.result = result;
            jsHelper.hideWaitDialog();
        }, error => {
            jsHelper.hideWaitDialog();
            self.isError = true;
        });
    }
}
