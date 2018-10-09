import { Component, Input, OnInit } from '@angular/core';
import { User, ReportedCompilerMessage } from './models';
import { HackingService } from './hacking.service';




declare var jsHelper: any;

@Component({
    selector: 'admin-compiler-messages',
    templateUrl: 'admin-compiler-messages.component.html'
})
export class AdminCompilerMessagesComponent implements OnInit {
    constructor(
        private hackingService: HackingService) { }

    messages: ReportedCompilerMessage[] = [];
    detail: ReportedCompilerMessage = <ReportedCompilerMessage>{};
    isLoading: boolean = true;

    ngOnInit(): void {
        var self = this;
        this.hackingService
            .getAdminReportedCompilerMessages()
            .subscribe(data => {
                self.messages = data;
                self.isLoading = false;
            }, error => {
            });
    }

    showDetails(obj: ReportedCompilerMessage): void {
        this.detail = obj;
    }
}
