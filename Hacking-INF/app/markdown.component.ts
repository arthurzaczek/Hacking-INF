﻿import { Component, Input, OnInit, OnChanges, SimpleChanges, AfterViewChecked, ElementRef } from '@angular/core';
declare var jsHelper: any;
@Component({
    selector: 'markdown',
    template: `<div [innerHTML]="convertedData"></div>`
})
export class MarkdownComponent implements OnChanges, OnInit, AfterViewChecked {
    @Input() data: string = "";
    convertedData: string;

    constructor(public element: ElementRef) {
    }  

    ngOnInit(): void {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (this.data != null) {
            var renderer = new marked.Renderer();
            renderer.table = function (header, body) {
                return '<table class="table table-striped">' + header + body + '</table>';
            };
            renderer.code = function (code, language) {
                var newCode = code.replace('\\stdin\{', '<span class="stdin">test</span>');
                return '<pre><code>' + newCode + '</code></pre>'; 
            };
            this.convertedData = marked(this.data, {
                renderer: renderer
            });
        }
    }

    public ngAfterViewChecked(): void {
        jsHelper.initHighlightJS();
    }  
}