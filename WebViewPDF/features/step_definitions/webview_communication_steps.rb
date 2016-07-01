Given(/^I am on the Home Screen$/) do
  current_page = page(HomeScreen).await()
  @current_page = current_page
end

When(/^I tap the "(.*?)" button$/) do |arg1|
  @current_page.tap_mock_image_button
end

Then(/^I expect the scan result to be "(.*?)"$/) do |arg1|
  @current_page.mock_image_succeeded arg1
end