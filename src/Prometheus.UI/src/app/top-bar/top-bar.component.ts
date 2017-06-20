import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
declare var jQuery: any;

@Component({
  selector: 'app-top-bar',
  templateUrl: './top-bar.component.html',
  styleUrls: ['./top-bar.component.css']
})
export class TopBarComponent implements OnInit {

  constructor(public auth: AuthService) {
    auth.handleAuthentication();
  }

  ngOnInit() {
  }

  toggleNavigation(): void {
    jQuery("body").toggleClass("mini-navbar");
    this.smoothlyMenu();
  }

  smoothlyMenu(): void {
    if (!jQuery('body').hasClass('mini-navbar') || jQuery('body').hasClass('body-small')) {
      // Hide menu in order to smoothly turn on when maximize menu
      jQuery('#side-menu').hide();
      // For smoothly turn on menu
      setTimeout(
        function() {
          jQuery('#side-menu').fadeIn(400);
        }, 200);
    } else if (jQuery('body').hasClass('fixed-sidebar')) {
      jQuery('#side-menu').hide();
      setTimeout(
        function() {
          jQuery('#side-menu').fadeIn(400);
        }, 100);
    } else {
      // Remove all inline style from jquery fadeIn function to reset menu state
      jQuery('#side-menu').removeAttr('style');
    }
  }
}
