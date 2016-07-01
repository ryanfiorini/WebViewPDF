require 'calabash-android/abase'

class HomeScreen < Calabash::ABase
  # including this prevents the setting of text in the 
  # username, password, and otp fields.
  #include BankMobile::AndroidHelpers

  def trait
    "webView css:'h1'"
  end

  def header_field
    trait
  end

  def content_field
    "webView css:'#txtContent'"
  end

  def mock_image_button
    "all webView css:'#mockImage'"
  end

  def tap_mock_image_button
    #wait_for_element_exists(mock_image_button)
    #scroll_and_touch_button(mock_image_button, :down)
    #touch(mock_image_button)
    #touch("webView css:'button#mockImage'")
    #touch("webView xpath:'//body/button[contains(@id, \"mockImage\")]'")
    evaluate_javascript("webview", "return document.querySelector('#mockImage').onclick()")
  end

  def mock_image_succeeded(content_value)
    # wait for scan value to return
    sleep(5)
    #query = query("webView css:'input' {value CONTAINS '#{content_value}'}")
    query = evaluate_javascript("webview", "return document.querySelector('#txtContent').value").first
    puts query
    fail "Unable to locate element \##{content_value}" if query.empty?
    query
  end

   def expect_text(text, timeout=5)
      begin
        mark = escape_quotes(text)
        wait_for_text("#{mark}", timeout: timeout)
        return true
      rescue
        false
      end
    end

    #scroll_until_i_see("save 28%",:down), scroll_until_i_see("There's Something I'",:up)
    def scroll_until_i_see(button, direction = :up)
      #wait_poll(until_exists: "* {text CONTAINS[c] '#{escape_quotes(text)}'}", timeout: 10) do
      wait_poll(until_exists: "webView css:'#mockImage'", timeout: 10) do
        pan("* id:'viewpager'", direction)
      end
    end

    def scroll_and_touch_button(button_to_touch, direction = :up)
      scroll_until_i_see(button_to_touch, direction)
      #touch("* marked:'#{text_to_touch}'")
      touch(button_to_touch)
    end
end
