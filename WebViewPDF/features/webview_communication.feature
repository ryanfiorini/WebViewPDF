Feature: As a user I would like to make sure that I am 
  able to cmmunicate from a webview to the native layer
  and then back to the webview

  Scenario: AS a user I want to scan a QR code
    Given I am on the Home Screen
    #Then I scroll down
    When I tap the "Mock Image Result" button
    Then I expect the scan result to be "QR Code Test"
